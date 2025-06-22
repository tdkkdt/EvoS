import {Button, LinearProgress, Stack} from "@mui/material";
import {reloadProxyConfig} from "../../lib/Evos";
import {FlexBox} from "../generic/BasicComponents";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "../generic/BaseDialog";
import React, {useState} from "react";
import {processError} from "../../lib/Error";
import {useNavigate} from "react-router-dom";

export default function Proxy() {
    const [msg, setMsg] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleClick = () => {
        setProcessing(true);
        const abort = new AbortController();
        reloadProxyConfig(abort, authHeader())
            .then(() => setMsg("Reloaded successfully"))
            .catch(e =>
            {
                if (e.response?.status === 404) {
                    setMsg("Reloading failed");
                } else {
                    processError(e, err => setMsg(err.text), navigate)
                }
            })
            .then(() => setProcessing(false));

        return () => abort.abort();
    }

    return <>
        <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
        <Stack>
            <FlexBox style={{ padding: 4, flexWrap: 'wrap' }}>
                <Button disabled={processing} onClick={() => handleClick()} focusRipple>Reload proxy config</Button>
            </FlexBox>
            {processing && <LinearProgress />}
        </Stack>
    </>;
}