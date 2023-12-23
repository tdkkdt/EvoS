import {Button, LinearProgress, Stack} from "@mui/material";
import {PendingShutdownType, scheduleShutdown} from "../../lib/Evos";
import {FlexBox} from "../generic/BasicComponents";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "../generic/BaseDialog";
import React, {useState} from "react";
import {processError} from "../../lib/Error";
import {useNavigate} from "react-router-dom";

export default function Shutdown() {
    const [msg, setMsg] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleClick = (type: PendingShutdownType) => {
        const text = type === PendingShutdownType.Now
            ? "Server is shutting down."
            : type === PendingShutdownType.WaitForGamesToEnd
                ? "Server will shut down once all currently running games have finished. No new games will be started."
                : type === PendingShutdownType.WaitForPlayersToLeave
                    ? "Server will shut down once all players have left."
                    : "Server shutdown cancelled";
        setProcessing(true);
        const abort = new AbortController();
        scheduleShutdown(abort, authHeader(), type)
            .then(() => setMsg(text))
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));

        return () => abort.abort();
    }

    return <>
        <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
        <Stack>
            <FlexBox style={{ padding: 4, flexWrap: 'wrap' }}>
                <Button disabled={processing} onClick={() => handleClick(PendingShutdownType.Now)} focusRipple>Shutdown now</Button>
                <Button disabled={processing} onClick={() => handleClick(PendingShutdownType.WaitForGamesToEnd)} focusRipple>Wait for games to end</Button>
                <Button disabled={processing} onClick={() => handleClick(PendingShutdownType.WaitForPlayersToLeave)} focusRipple>Wait for players to leave</Button>
                <Button disabled={processing} onClick={() => handleClick(PendingShutdownType.None)} focusRipple>Cancel shutdown</Button>
            </FlexBox>
            {processing && <LinearProgress />}
        </Stack>
    </>;
}