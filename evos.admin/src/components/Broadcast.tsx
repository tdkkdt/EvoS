import {Box, Button, LinearProgress, TextField} from "@mui/material";
import {broadcast} from "../lib/Evos";
import React, {useState} from "react";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate} from "react-router-dom";
import {processError} from "../lib/Error";
import BaseDialog from "./BaseDialog";

export default function Broadcast() {
    const [msg, setMsg] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        const message = data.get('message') as string;

        if (!message) {
            return;
        }

        setProcessing(true);
        broadcast(authHeader(), message)
            .then(() => setMsg("Message sent"))
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));
    };

    return <>
        <Box component="form" onSubmit={handleSubmit} noValidate style={{ padding: 4 }}>
            <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
            <TextField
                margin="normal"
                required
                fullWidth
                id="message"
                label="Message"
                name="message"
                autoFocus
            />
            <Button
                disabled={processing}
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
            >
                Broadcast
            </Button>
            {processing && <LinearProgress />}
        </Box>
    </>;
}