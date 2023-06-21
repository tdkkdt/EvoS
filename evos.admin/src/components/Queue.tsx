import {GroupData, PlayerData, QueueData} from "../lib/Evos";
import {Typography} from "@mui/material";
import Group from "./Group";
import {FlexBox} from "./BasicComponents";

interface Props {
    info: QueueData;
    groupData: Map<number, GroupData>;
    playerData: Map<number, PlayerData>;
}

function Queue({info, groupData, playerData}: Props) {
    return <>
        <Typography variant={'h3'}>{info.type}</Typography>
        <FlexBox style={{ padding: 4 }}>
            {info.groupIds.map((groupId) => {
                const info = groupData.get(groupId);
                return info && <Group key={`group_${groupId}`} info={info} playerData={playerData} />;
            })}
        </FlexBox>
    </>;
}

export default Queue;