import {Button, LinearProgress, Stack} from "@mui/material";
import {pauseQueue} from "../lib/Evos";
import {FlexBox} from "./BasicComponents";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "./BaseDialog";
import React, {useState} from "react";
import {processError} from "../lib/Error";
import {useNavigate} from "react-router-dom";

export default function PauseQueue() {
    const [msg, setMsg] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleClick = (paused: boolean) => {
        const text = paused ? "Queue is paused" : "Queue is unpaused";
        setProcessing(true);
        const abort = new AbortController();
        pauseQueue(abort, authHeader(), paused)
            .then(() => setMsg(text))
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));

        return () => abort.abort();
    }

    return <>
        <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
        <Stack>
            <FlexBox style={{ padding: 4, flexWrap: 'wrap' }}>
                <Button disabled={processing} onClick={() => handleClick(true)} focusRipple>Pause queue</Button>
                <Button disabled={processing} onClick={() => handleClick(false)} focusRipple>Unpause queue</Button>
            </FlexBox>
            {processing && <LinearProgress />}
        </Stack>
    </>;
}