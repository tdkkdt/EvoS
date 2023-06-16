import React, {useEffect, useState} from 'react';
import {getStatus, GroupData, PlayerData, Status} from "../lib/Evos";
import {LinearProgress} from "@mui/material";
import Queue from "./Queue";

function StatusPage() {
    const [loading, setLoading] = useState(true);
    // const [error, setError] = useState<string>();
    const [status, setStatus] = useState<Status>();
    const [players, setPlayers] = useState<Map<number, PlayerData>>();
    const [groups, setGroups] = useState<Map<number, GroupData>>();

    useEffect(() => {
        console.log("loading data");
        getStatus()
            .then((resp) => {
                console.log("loaded data");
                setStatus(resp.data);
                setLoading(false);
            })
            .catch((error) => {
                console.log("failed loading data");
                // Error
                if (error.response) {
                    // The request was made and the server responded with a status code
                    // that falls out of the range of 2xx
                    // console.log(error.response.data);
                    // console.log(error.response.status);
                    // console.log(error.response.headers);
                } else if (error.request) {
                    // The request was made but no response was received
                    // `error.request` is an instance of XMLHttpRequest in the
                    // browser and an instance of
                    // http.ClientRequest in node.js
                    console.log(error.request);
                } else {
                    // Something happened in setting up the request that triggered an Error
                    console.log('Error', error.message);
                }
                console.log(error.config);
            })
    }, [])

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
    }, [status])

    const queuedGroups = new Set(status?.queues?.flatMap(q => q.groupIds));
    const notQueuedGroups = groups && [...groups.keys()].filter(g => !queuedGroups.has(g));

    return (
        <div className="App">
            <header className="App-header">
                {loading && <LinearProgress />}
                {notQueuedGroups && groups && players
                    && <Queue key={'not_queued'} info={{type: "Not queued", groupIds: notQueuedGroups}} groupData={groups} playerData={players} />}
                {status && groups && players
                    && status.queues.map(q => <Queue key={q.type} info={q} groupData={groups} playerData={players} />)}
            </header>
        </div>
    );
}

export default StatusPage;
