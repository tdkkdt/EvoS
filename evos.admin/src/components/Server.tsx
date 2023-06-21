import {GameData, PlayerData, ServerData} from "../lib/Evos";
import {Tooltip, Typography} from "@mui/material";
import Game from "./Game";

interface Props {
    info: ServerData;
    game?: GameData;
    playerData: Map<number, PlayerData>;
}

export default function Server({info, game, playerData}: Props) {
    return <>
        <Tooltip arrow title={info.id}><Typography variant={'h3'}>{info.name}</Typography></Tooltip>
        {game && <Game info={game} playerData={playerData} />}
    </>;
}