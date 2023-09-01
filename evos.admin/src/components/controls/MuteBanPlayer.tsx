import {
    Box,
    Button,
    LinearProgress,
    MenuItem,
    Select,
    SelectChangeEvent,
    Stack,
    TextField,
    Typography
} from "@mui/material";
import React, {useState} from "react";
import {processError} from "../../lib/Error";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "../generic/BaseDialog";
import {useNavigate} from "react-router-dom";
import {AxiosResponse} from "axios";
import {cap, PenaltyInfo} from "../../lib/Evos";
import {EvosCard} from "../generic/BasicComponents";

interface MutePlayerProps {
    disabled: boolean;
    deadline?: Date;
    accountId: number;
    action: (authHeader: string, penaltyInfo: PenaltyInfo) => Promise<AxiosResponse>;
    handle: string;
    actionText: string;
    doneText: string;
    onCommit: () => void;
}

const DEFAULT = 30;

export default function MuteBanPlayer({disabled, deadline, accountId, action, handle, actionText, doneText, onCommit}: MutePlayerProps) {
    const [durationMinutes, setDurationMinutes] = useState<number>(DEFAULT);
    const [description, setDescription] = useState<string>("");
    const [processing, setProcessing] = useState(false);
    const [msg, setMsg] = useState<string>();

    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        const description = data.get('description') as string;

        if (!description) {
            return;
        }

        setProcessing(true);
        const penaltyInfo: PenaltyInfo = {
            accountId: accountId,
            durationMinutes: durationMinutes,
            description: description,
        };
        action(authHeader(), penaltyInfo)
            .then(() => {
                setMsg(`${handle} has been ${durationMinutes ? "" : "un"}${doneText}`);
                setDescription("");
                if (!durationMinutes) {
                    setDurationMinutes(DEFAULT);
                }
            })
            .catch(e => processError(e, err => setMsg(err.text), navigate))
            .then(() => setProcessing(false));
    };

    const handleDismiss = () => {
        setMsg(undefined);
        onCommit();
    }

    const handleUpdateDuration = (event: SelectChangeEvent) => {
        setDurationMinutes(parseInt(event.target.value));
    };

    return <EvosCard variant="outlined">
        <Stack direction={'column'}>
            {deadline && <Stack direction={'row'}>
                <Typography variant={'body1'} style={{width: '100%'}}>{`${cap(doneText)} until ${deadline.toLocaleString()}`}</Typography>
            </Stack>}
            <Box component="form" onSubmit={handleSubmit} noValidate sx={{mt: 1}}>
                <BaseDialog title={msg} onDismiss={handleDismiss} />
                <Typography variant={'h5'} style={{ textTransform: 'capitalize' }}>{actionText}</Typography>
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="description"
                    label="Description"
                    name="description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
                <Select
                    id="duration"
                    value={`${durationMinutes}`}
                    label={`${actionText} for`}
                    onChange={handleUpdateDuration}
                    fullWidth
                >
                    {deadline && <MenuItem value={0}>{`Un${actionText}`}</MenuItem>}
                    <MenuItem value={15}>15 min</MenuItem>
                    <MenuItem value={30}>30 min</MenuItem>
                    <MenuItem value={60}>1 hour</MenuItem>
                    <MenuItem value={180}>3 hours</MenuItem>
                    <MenuItem value={720}>12 hours</MenuItem>
                    <MenuItem value={1440}>1 day</MenuItem>
                    <MenuItem value={4320}>3 days</MenuItem>
                    <MenuItem value={10080}>A week</MenuItem>
                    <MenuItem value={43200}>A month</MenuItem>
                    <MenuItem value={525600}>A year</MenuItem>
                    <MenuItem value={52596000}>A century</MenuItem>
                </Select>
                <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    sx={{mt: 3, mb: 2}}
                    disabled={disabled || processing || !description || !!msg}
                >
                    {`${durationMinutes ? "" : "un"}${actionText}`}
                </Button>
                {processing && <LinearProgress />}
            </Box>
        </Stack>
    </EvosCard>;
}