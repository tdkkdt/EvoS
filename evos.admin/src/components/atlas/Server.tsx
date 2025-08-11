import {useState} from "react";
import {
    GameData,
    PlayerData,
    ServerData,
    shutdownServer,
} from "../../lib/Evos";
import {
    Box,
    Button,
    Tooltip,
    Typography,
} from "@mui/material";
import Game from "./Game";
import {useAuthHeader} from "react-auth-kit";
import BaseDialog from "../generic/BaseDialog";
import {processError} from "../../lib/Error";
import {useNavigate} from "react-router-dom";

interface Props {
    info: ServerData;
    game?: GameData;
    playerData: Map<number, PlayerData>;
}

function buildTitle(info: ServerData, game?: GameData) {
    let suffix = "";
    if (game) {
        const subType = game.gameSubType.split('@')[0];
        suffix = ` - ${game.gameType} ${subType}`
    }
    return info.name + suffix;
}

export default function Server({info, game, playerData}: Props) {
    const [msg, setMsg] = useState<string>();
    const [processing, setProcessing] = useState<boolean>();
    const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const shutdownServerHandler = () => {
        setProcessing(true);

        const abort = new AbortController();
        shutdownServer(abort, authHeader(), info.id)
            .then(() => setMsg("Server shutdown initiated."))
            .catch((e) => processError(e, (err) => setMsg(err.text), navigate))

        return () => abort.abort();
    };

    return (
        <>
            <Box
                display="flex"
                alignItems="center"
                gap={2}
                justifyContent="center"
                sx={{p: 2}}
            >
                <BaseDialog title={msg} onDismiss={() => setMsg(undefined)}/>
                <BaseDialog
                    title="Confirm Server Shutdown"
                    content={`Are you sure you want to shutdown ${info.name}?`}
                    onDismiss={() => setShowConfirmDialog(false)}
                    onAccept={() => {
                        setShowConfirmDialog(false);
                        shutdownServerHandler();
                    }}
                    acceptText="Stop game"
                    dismissText="Cancel"
                    props={{open: showConfirmDialog}}
                />
                <Tooltip arrow title={info.id}>
                    <Typography variant="h3">{buildTitle(info, game)}</Typography>
                </Tooltip>
                {game && info.name !== "Custom game" && (
                    <Button
                        disabled={processing}
                        variant="contained"
                        color="primary"
                        onClick={() => setShowConfirmDialog(true)}
                    >
                        Stop game
                    </Button>
                )}
            </Box>
            {game && <Game info={game} playerData={playerData} expanded/>}
        </>
    );
}