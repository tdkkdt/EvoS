import {Button, LinearProgress, Stack} from "@mui/material";
import {generateTempPassword} from "../../lib/Evos";
import {FlexBox} from "../generic/BasicComponents";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "../generic/BaseDialog";
import React, {useState} from "react";
import {processError} from "../../lib/Error";
import {useNavigate} from "react-router-dom";

interface Props {
    accountId: number,
}

export default function TempPassword({accountId}: Props) {
    const [msg, setMsg] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleClick = () => {
        setProcessing(true);
        const abort = new AbortController();
        generateTempPassword(abort, authHeader(), accountId)
            .then((resp) => setMsg(resp.data.code))
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));

        return () => abort.abort();
    }

    return <>
        <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
        <Stack>
            <FlexBox style={{ padding: 4, flexWrap: 'wrap' }}>
                <Button disabled={processing && accountId !== 0} onClick={handleClick} focusRipple>Generate temporary password</Button>
            </FlexBox>
            {processing && <LinearProgress />}
        </Stack>
    </>;
}