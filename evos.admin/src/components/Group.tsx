import {GroupData, PlayerData} from "../lib/Evos";
import {List, ListItem} from "@mui/material";
import Player from "./Player";

interface Props {
    info: GroupData;
    playerData: Map<number, PlayerData>;
}

function Group({info, playerData}: Props) {
    return <>
        <List>
            {info.accountIds.map((accountId) =>
                <ListItem disablePadding key={`player_${accountId}`}>
                    <Player info={playerData.get(accountId)} />
                </ListItem>)}
        </List>
    </>;
}

export default Group;