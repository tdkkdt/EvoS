import {Box, Button, LinearProgress, MenuItem, Select, SelectChangeEvent, TextField, Typography} from "@mui/material";
import React, {useState} from "react";
import {processError} from "../lib/Error";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "./BaseDialog";
import {useNavigate} from "react-router-dom";
import {AxiosResponse} from "axios";
import {PenaltyInfo} from "../lib/Evos";
import {EvosCard} from "./BasicComponents";

interface MutePlayerProps {
    disabled: boolean;
    accountId: number;
    action: (authHeader: string, penaltyInfo: PenaltyInfo) => Promise<AxiosResponse>;
    actionText: string;
    doneText: string;
}

export default function MuteBanPlayer({disabled, accountId, action, actionText, doneText}: MutePlayerProps) {
    const [durationMinutes, setDurationMinutes] = useState<number>(30);
    const [processing, setProcessing] = useState(false);
    const [valid, setValid] = useState(false);
    const [msg, setMsg] = useState<string>();

    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        const description = data.get('description') as string;

        if (!durationMinutes || !description) {
            return;
        }

        setProcessing(true);
        const penaltyInfo: PenaltyInfo = {
            accountId: accountId,
            durationMinutes: durationMinutes,
            description: description,
        };
        action(authHeader(), penaltyInfo)
            .then(() => setMsg(doneText))
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));
    };

    const handleUpdateDuration = (event: SelectChangeEvent) => {
        setDurationMinutes(parseInt(event.target.value));
    };

    return <EvosCard variant="outlined">
            <Box component="form" onSubmit={handleSubmit} noValidate sx={{mt: 1}}>
            <BaseDialog title={msg} onDismiss={() => setMsg(undefined)} />
            <Typography variant={'h5'}>{actionText}</Typography>
            <TextField
                margin="normal"
                required
                fullWidth
                id="description"
                label="Description"
                name="description"
                onChange={(e) => setValid(!!e.target.value)}
            />
            <Select
                id="duration"
                value={`${durationMinutes}`}
                label={`${actionText} for`}
                onChange={handleUpdateDuration}
                fullWidth
            >
                <MenuItem value={15}>15 min</MenuItem>
                <MenuItem value={30}>30 min</MenuItem>
                <MenuItem value={60}>1 hour</MenuItem>
                <MenuItem value={180}>3 hours</MenuItem>
                <MenuItem value={720}>12 hours</MenuItem>
                <MenuItem value={1440}>1 day</MenuItem>
                <MenuItem value={4320}>3 days</MenuItem>
            </Select>
            <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{mt: 3, mb: 2}}
                disabled={disabled || processing || !valid}
            >
                {actionText}
            </Button>
            {processing && <LinearProgress />}
        </Box>
    </EvosCard>;
}