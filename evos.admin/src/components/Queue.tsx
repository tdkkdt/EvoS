import {GroupData, PlayerData, QueueData} from "../lib/Evos";
import {List, ListItem} from "@mui/material";
import Group from "./Group";

interface Props {
    info: QueueData;
    groupData: Map<number, GroupData>;
    playerData: Map<number, PlayerData>;
}

function Queue({info, groupData, playerData}: Props) {
    return <>
        <h2>{info.type}</h2>
        <List>
            {info.groupIds.map((groupId) => {
                const info = groupData.get(groupId);
                return <>
                    <ListItem disablePadding key={`group_${groupId}`}>
                        {info && <Group info={info} playerData={playerData} />}
                    </ListItem>
                </>;
            })}
        </List>
    </>;
}

export default Queue;