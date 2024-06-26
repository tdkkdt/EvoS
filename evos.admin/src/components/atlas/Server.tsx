import {GameData, PlayerData, ServerData} from "../../lib/Evos";
import {Tooltip, Typography} from "@mui/material";
import Game from "./Game";

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
    return <>
        <Tooltip arrow title={info.id}><Typography variant={'h3'}>{buildTitle(info, game)}</Typography></Tooltip>
        {game && <Game info={game} playerData={playerData} expanded />}
    </>;
}