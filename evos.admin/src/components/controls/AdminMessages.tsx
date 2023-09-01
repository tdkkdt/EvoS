import {Box, Button, LinearProgress, Table, TableBody, TableCell, TableHead, TableRow, TextField} from "@mui/material";
import {AdminMessage, sendAdminMessage, formatDate, getAdminMessages} from "../../lib/Evos";
import React, {useEffect, useState} from "react";
import {useAuthHeader} from "react-auth-kit";
import {useNavigate} from "react-router-dom";
import {EvosError, processError} from "../../lib/Error";
import {EvosCard, FlexBox, plainAccountLink} from "../generic/BasicComponents";
import ErrorDialog from "../generic/ErrorDialog";


interface AdminMessagesProps {
    accountId: number;
}

export default function AdminMessages({accountId}: AdminMessagesProps) {
    const [processing, setProcessing] = useState<boolean>();
    const [error, setError] = useState<EvosError>();
    const [messages, setMessages] = useState<AdminMessage[]>();
    const [updateTs, setUpdateTs] = useState<Date>(new Date());

    const authHeader = useAuthHeader()();
    const navigate = useNavigate();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        const msg = data.get('message') as string;

        if (!msg) {
            return;
        }

        setProcessing(true);
        const abort = new AbortController();
        sendAdminMessage(abort, authHeader, accountId, msg)
            .catch(e => processError(e, setError, navigate))
            .then(() => {
                setProcessing(false);
                setUpdateTs(new Date(new Date().getTime())); // trigger reload
            });

        return () => abort.abort();
    };

    useEffect(() => {
        if (!accountId) return;
        const abort = new AbortController();
        getAdminMessages(abort, authHeader, accountId)
            .then((resp) => {
                setMessages(resp.data.entries);
            })
            .catch((error) => processError(error, setError, navigate));

        return () => abort.abort();
    }, [authHeader, navigate, accountId, updateTs]);

    return <FlexBox style={{ flexDirection: 'column' }}>
        {error && <ErrorDialog error={error} onDismiss={() => setError(undefined)} />}
        <EvosCard variant="outlined">
            <Box component="form" onSubmit={handleSubmit} noValidate style={{ padding: 4 }}>
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="message"
                    label="Message"
                    name="message"
                    multiline
                    autoFocus
                />
                <Button
                    disabled={processing}
                    type="submit"
                    fullWidth
                    variant="contained"
                    sx={{ mt: 3, mb: 2 }}
                >
                    Send
                </Button>
                {processing && <LinearProgress />}
            </Box>
        </EvosCard>

        {messages &&
            <Box style={{ margin: "0 auto" }}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>From</TableCell>
                            <TableCell>Test</TableCell>
                            <TableCell>Sent at</TableCell>
                            <TableCell>Viewed at</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {messages.map((row) => {
                            return <TableRow
                                key={row.sentAt}
                                sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                            >
                                <TableCell>{plainAccountLink(row.from, row.fromHandle, navigate)}</TableCell>
                                <TableCell>{row.text}</TableCell>
                                <TableCell>{formatDate(row.sentAt)}</TableCell>
                                <TableCell>{formatDate(row.viewedAt)}</TableCell>
                            </TableRow>
                        })}
                    </TableBody>
                </Table>
            </Box>
        }
    </FlexBox>;
}