import {Box, Button, LinearProgress, MenuItem, Select, SelectChangeEvent, TextField, Typography} from "@mui/material";
import {
    EvosServerMessageSeverity,
    EvosServerMessageType,
    getMotd,
    Language, makeServerMsgData,
    MessagesWithMetadata,
    setMotd,
    toMap
} from "../../lib/Evos";
import React, {useEffect, useState} from "react";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate} from "react-router-dom";
import {processError} from "../../lib/Error";
import BaseDialog from "../generic/BaseDialog";

interface ServerMessageProps {
    type: EvosServerMessageType;
}

export default function ServerMessage({type}: ServerMessageProps) {
    const [msg, setMsg] = useState<string>();
    const [serverMessage, setServerMessage] = useState<Map<Language,string>>();
    const [serverMessageSeverity, setServerMessageSeverity] = useState<EvosServerMessageSeverity>(EvosServerMessageSeverity.Warning);
    const [loading, setLoading] = useState<boolean>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    useEffect(() => {
        setLoading(true);
        const abort = new AbortController();
        getMotd(abort, type)
            .then((resp) => {
                setServerMessage(toMap(
                    Object.keys(Language),
                    lg => lg as Language,
                    lg => resp.data.msg[lg] as string));
                setServerMessageSeverity(resp.data.severity || EvosServerMessageSeverity.Warning);
            })
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setLoading(false));

        return () => abort.abort();
    }, [navigate, type]);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>, lg: Language) => {
        setServerMessage(message => {
            const msg = new Map<Language, string>(message);
            msg.set(lg, event.target.value);
            return msg;
        });
    };

    const handleUpdateSeverity = (event: SelectChangeEvent) => {
        setServerMessageSeverity(event.target.value as EvosServerMessageSeverity);
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        setProcessing(true);
        if (!serverMessage) {
            return;
        }

        const abort = new AbortController();
        setMotd(abort, authHeader(), type, makeServerMsgData(serverMessage, serverMessageSeverity))
            .then(() => setMsg("Saved"))
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));

        return () => abort.abort();
    };

    const isValid = !!serverMessage
        && (!!serverMessage.get(Language.en)
            || Object.keys(Language).reduce((res, lg) => res && !serverMessage.get(lg as Language), true))

    return <>
        <Box component="form" onSubmit={handleSubmit} noValidate style={{ padding: 4 }}>
            <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
            <Typography variant="h4">{type}</Typography>
            {serverMessage && Object.keys(Language).map(lg => <TextField
                margin="normal"
                disabled={processing || loading}
                required={lg === Language.en}
                fullWidth
                id={lg}
                label={lg}
                name={lg}
                key={lg}
                multiline
                autoFocus={lg === Language.en}
                placeholder={serverMessage.get(Language.en)}
                value={serverMessage.get(lg as Language) || ""}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => handleChange(e, lg as Language)}
            />)}

            {MessagesWithMetadata.has(type) && <Select
                id="severity"
                value={serverMessageSeverity as string}
                label="Severity"
                onChange={handleUpdateSeverity}
                fullWidth
            >
                {Object.keys(EvosServerMessageSeverity).map(s => <MenuItem value={s} key={s}>{s}</MenuItem>)}
            </Select>}

            <Button
                disabled={processing || loading || !isValid}
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
            >
                Save
            </Button>
            {(processing || loading) && <LinearProgress />}
        </Box>
    </>;
}