import React, {useEffect, useState} from 'react';
import {ban, getPlayer, mute, PlayerDetails} from "../../lib/Evos";
import {EvosError, processError} from "../../lib/Error";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate, useParams} from "react-router-dom";
import Player from "../Player";
import {LinearProgress, Paper} from "@mui/material";
import ErrorDialog from "../ErrorDialog";
import MuteBanPlayer from "../MuteBanPlayer";
import {EvosCard, StackWrapper} from "../BasicComponents";


export default function ProfilePage() {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<EvosError>();
    const [playerDetails, setPlayerDetails] = useState<PlayerDetails>();

    const {accountId} = useParams();
    const accountIdNumber = accountId && parseInt(accountId);

    const authHeader = useAuthHeader()();
    const navigate = useNavigate();

    useEffect(() => {
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
        <Paper>
            {error && <ErrorDialog error={error} onDismiss={() => setError(undefined)} />}
            <StackWrapper>
                <EvosCard variant="outlined"><Player info={playerDetails?.player} /></EvosCard>
                {loading && <LinearProgress />}
                <MuteBanPlayer
                    disabled={loading}
                    accountId={playerDetails?.player.accountId ?? 0}
                    action={mute}
                    actionText={"Mute"}
                    doneText={`${playerDetails?.player.handle ?? "Nobody"} has been muted`}
                />
                <MuteBanPlayer
                    disabled={loading}
                    accountId={playerDetails?.player.accountId ?? 0}
                    action={ban}
                    actionText={"Ban"}
                    doneText={`${playerDetails?.player.handle ?? "Nobody"} has been banned`}
                />
            </StackWrapper>
        </Paper>
    );
}
