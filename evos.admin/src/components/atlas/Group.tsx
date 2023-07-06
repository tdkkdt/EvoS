import {GroupData, PlayerData} from "../../lib/Evos";
import {List, ListItem} from "@mui/material";
import Player from "./Player";

interface Props {
    info: GroupData;
    playerData: Map<number, PlayerData>;
    hidePlayers?: Set<number>;
}

function Group({info, playerData, hidePlayers}: Props) {
    return <>
        <List style={{ padding: 4 }}>
            {info.accountIds.map((accountId) =>
                <ListItem disablePadding key={`player_${accountId}`}>
                    <Player info={playerData.get(accountId)} greyOut={hidePlayers && hidePlayers.has(accountId)} />
                </ListItem>)}
        </List>
    </>;
}

export default Group;