import React, {useEffect, useState} from 'react';
import {GameData, getStatus, GroupData, PlayerData, Status} from "../lib/Evos";
import {LinearProgress} from "@mui/material";
import Queue from "./Queue";
import {useAuthHeader} from "react-auth-kit";
import Server from "./Server";
import {useNavigate} from "react-router-dom";

function StatusPage() {
    const [loading, setLoading] = useState(true);
    // const [error, setError] = useState<string>();
    const [status, setStatus] = useState<Status>();
    const [players, setPlayers] = useState<Map<number, PlayerData>>();
    const [groups, setGroups] = useState<Map<number, GroupData>>();
    const [games, setGames] = useState<Map<string, GameData>>();

    const authHeader = useAuthHeader()();
    const navigate = useNavigate();

    useEffect(() => {
        console.log("loading data");
        getStatus(authHeader)
            .then((resp) => {
                console.log("loaded data");
                setStatus(resp.data);
                setLoading(false);
            })
            .catch((error) => {
                console.log("failed loading data");
                if (error.response?.status === 401) {
                    navigate("/login")
                }
                else if (error.response?.status === 404) {
                    console.log("404");
                }
            })
    }, [authHeader, navigate])

    useEffect(() => {
        if (!status) {
            console.log("no data");
            return;
        }
        console.log("processing data");
        const _players = status.players.reduce((res, p) => {
            res.set(p.accountId, p);
            return res;
        }, new Map<number, PlayerData>());
        setPlayers(_players);
        const _groups = status.groups.reduce((res, g) => {
            res.set(g.groupId, g);
            return res;
        }, new Map<number, GroupData>());
        setGroups(_groups);
        const _games = status.games.reduce((res, g) => {
            res.set(g.server, g);
            return res;
        }, new Map<string, GameData>());
        setGames(_games);
    }, [status])

    const queuedGroups = new Set(status?.queues?.flatMap(q => q.groupIds));
    const notQueuedGroups = groups && [...groups.keys()].filter(g => !queuedGroups.has(g));

    return (
        <div className="App">
            <header className="App-header">
                {loading && <LinearProgress />}
                {status && players && games
                    && status.servers
                        .sort((s1, s2) => s1.name.localeCompare(s2.name))
                        .map(s => <Server info={s} game={games.get(s.id)} playerData={players}/>)}
                {status && groups && players
                    && status.queues.map(q => <Queue key={q.type} info={q} groupData={groups} playerData={players} />)}
                {notQueuedGroups && groups && players
                    && <Queue key={'not_queued'} info={{type: "Not queued", groupIds: notQueuedGroups}} groupData={groups} playerData={players} />}
            </header>
        </div>
    );
}

export default StatusPage;
