import {GroupData, PlayerData, QueueData} from "../../lib/Evos";
import {Typography} from "@mui/material";
import Group from "./Group";
import {FlexBox} from "../generic/BasicComponents";

interface Props {
    info: QueueData;
    groupData: Map<number, GroupData>;
    playerData: Map<number, PlayerData>;
    hidePlayers?: Set<number>;
}

function Queue({info, groupData, playerData, hidePlayers}: Props) {
    return <>
        <Typography variant={'h3'}>{info.type}</Typography>
        <FlexBox style={{ flexWrap: 'wrap' }}>
            {info.groupIds.map((groupId) => {
                const info = groupData.get(groupId);
                const hidden = info && hidePlayers && !info.accountIds.some(accId => !hidePlayers.has(accId))
                return info && !hidden && <Group key={`group_${groupId}`} info={info} playerData={playerData} />;
            })}
        </FlexBox>
    </>;
}

export default Queue;