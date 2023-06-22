import {Box, Button, TextField} from "@mui/material";
import {broadcast} from "../lib/Evos";
import React from "react";
import {useAuthHeader} from "react-auth-kit";

export default function Broadcast() {
    const authHeader = useAuthHeader();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        const message = data.get('message') as string;

        if (!message) {
            return;
        }

        broadcast(authHeader(), message);
    };

    return <>
        <Box component="form" onSubmit={handleSubmit} noValidate style={{ padding: 4 }}>
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
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
            >
                Broadcast
            </Button>
        </Box>
    </>;
}