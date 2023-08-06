import {Box, Button, LinearProgress, TextField} from "@mui/material";
import {issueRegistrationCode} from "../../lib/Evos";
import React, {useState} from "react";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate} from "react-router-dom";
import {processError} from "../../lib/Error";
import BaseDialog from "../generic/BaseDialog";

export default function IssueRegistrationCode() {
    const [code, setCode] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        const issueFor = data.get('issueFor') as string;

        if (!issueFor) {
            return;
        }

        setProcessing(true);
        const abort = new AbortController();
        issueRegistrationCode(abort, authHeader(), {issueFor: issueFor})
            .then((resp) => setCode(resp.data.code))
            .catch(e => processError(e, err => setCode(err.text), navigate))
            .then(() => setProcessing(false));

        return () => abort.abort();
    };

    return <>
        <Box component="form" onSubmit={handleSubmit} noValidate style={{ padding: 4 }}>
            <BaseDialog title={code} onDismiss={() => setCode(undefined)} />
            <TextField
                margin="normal"
                required
                fullWidth
                id="issueFor"
                label="Issue for"
                name="issueFor"
                autoFocus
            />
            <Button
                disabled={processing}
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
            >
                Issue registration code
            </Button>
            {processing && <LinearProgress />}
        </Box>
    </>;
}