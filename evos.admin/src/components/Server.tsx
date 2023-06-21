import {GameData, PlayerData, ServerData} from "../lib/Evos";
import {Typography} from "@mui/material";
import Game from "./Game";

interface Props {
    info: ServerData;
    game?: GameData;
    playerData: Map<number, PlayerData>;
}

export default function Server({info, game, playerData}: Props) {
    return <>
        <Typography variant={'h3'}>{info.name}</Typography>
        <Typography variant={'caption'}>{info.id}</Typography>
        {game && <Game info={game} playerData={playerData} />}
    </>;
}