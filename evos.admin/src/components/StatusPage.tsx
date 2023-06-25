import React, {useMemo, useState} from 'react';
import {getStatus, Status} from "../lib/Evos";
import {LinearProgress} from "@mui/material";
import Queue from "./Queue";
import {useAuthHeader} from "react-auth-kit";
import Server from "./Server";
import {useNavigate} from "react-router-dom";
import {EvosError, processError} from "../lib/Error";
import ErrorDialog from "./ErrorDialog";
import useInterval from "../lib/useInterval";
import useHasFocus from "../lib/useHasFocus";

function GroupBy<V, K>(key: (item: V) => K, list?: V[]) {
    return list?.reduce((res, p) => {
        res.set(key(p), p);
        return res;
    }, new Map<K, V>())
}

const UPDATE_PERIOD_MS = 20000;

function StatusPage() {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<EvosError>();
    const [status, setStatus] = useState<Status>();

    const authHeader = useAuthHeader()();
    const navigate = useNavigate();

    const players = useMemo(() => GroupBy(p => p.accountId, status?.players), [status]);
    const groups = useMemo(() => GroupBy(g => g.groupId, status?.groups), [status]);
    const games = useMemo(() => GroupBy(g => g.server, status?.games), [status]);

    const updatePeriodMs = useHasFocus() ? UPDATE_PERIOD_MS : undefined;

    useInterval(() => {
        getStatus(authHeader)
            .then((resp) => {
                setStatus(resp.data);
                setLoading(false);
            })
            .catch((error) => processError(error, setError, navigate))
    }, updatePeriodMs);

    const queuedGroups = new Set(status?.queues?.flatMap(q => q.groupIds));
    const notQueuedGroups = groups && [...groups.keys()].filter(g => !queuedGroups.has(g));
    const inGame = games && new Set([...games.values()]
        .flatMap(g => [...g.teamA, ...g.teamB])
        .map(t => t.accountId));

    return (
        <div className="App">
            <header className="App-header">
                {loading && <LinearProgress />}
                {error && <ErrorDialog error={error} onDismiss={() => setError(undefined)} />}
                {status && players && games
                    && status.servers
                        .sort((s1, s2) => s1.name.localeCompare(s2.name))
                        .map(s => <Server key={s.id} info={s} game={games.get(s.id)} playerData={players}/>)}
                {status && groups && players
                    && status.queues.map(q => <Queue key={q.type} info={q} groupData={groups} playerData={players} />)}
                {notQueuedGroups && groups && players && inGame
                    && <Queue
                        key={'not_queued'}
                        info={{type: "Not queued", groupIds: notQueuedGroups}}
                        groupData={groups}
                        playerData={players}
                        hidePlayers={inGame}
                    />}
            </header>
        </div>
    );
}

export default StatusPage;
