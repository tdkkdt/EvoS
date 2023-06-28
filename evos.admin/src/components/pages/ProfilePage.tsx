import React, {useEffect, useState} from 'react';
import {ban, getPlayer, mute, PlayerDetails} from "../../lib/Evos";
import {EvosError, processError} from "../../lib/Error";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate, useParams} from "react-router-dom";
import Player from "../Player";
import {LinearProgress, Stack} from "@mui/material";
import ErrorDialog from "../ErrorDialog";
import MuteBanPlayer from "../MuteBanPlayer";


export default function ProfilePage() {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<EvosError>();
    const [playerDetails, setPlayerDetails] = useState<PlayerDetails>();

    const {accountId} = useParams();
    const accountIdNumber = accountId && parseInt(accountId);
    console.log('load', accountId);

    const authHeader = useAuthHeader()();
    const navigate = useNavigate();

    useEffect(() => {
        console.log('useEffect', accountIdNumber);
        if (!accountIdNumber) return;
        setLoading(true);
        getPlayer(authHeader, accountIdNumber)
            .then((resp) => {
                setPlayerDetails(resp.data);
                setLoading(false);
            })
            .catch((error) => processError(error, setError, navigate))
    }, [accountIdNumber, authHeader, navigate, setPlayerDetails]);

    return (
        <div className="App">
            <header className="App-header">
                {error && <ErrorDialog error={error} onDismiss={() => setError(undefined)} />}
                <Stack>
                    <Player info={playerDetails?.player} />
                    {loading && <LinearProgress />}
                    <MuteBanPlayer
                        disabled={loading}
                        accountId={playerDetails?.player.accountId ?? 0}
                        action={mute}
                        actionText={"Mute"}
                        doneText={`${playerDetails?.player.handle ?? "Nobody"} is muted`}
                    />
                    <MuteBanPlayer
                        disabled={loading}
                        accountId={playerDetails?.player.accountId ?? 0}
                        action={ban}
                        actionText={"Ban"}
                        doneText={`${playerDetails?.player.handle ?? "Nobody"} is banned`}
                    />
                </Stack>
            </header>
        </div>
    );
}
