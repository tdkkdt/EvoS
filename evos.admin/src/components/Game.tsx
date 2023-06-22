import {GameData, GamePlayerData, PlayerData} from "../lib/Evos";
import {Box, ButtonBase, styled, Tooltip, Typography} from "@mui/material";
import {BgImage, FlexBox} from "./BasicComponents";
import {characterIcon, mapMiniPic} from "../lib/Resources";

interface Props {
    info: GameData;
    playerData: Map<number, PlayerData>;
}

export const TeamFlexBox = styled(FlexBox)(({ theme }) => ({
    paddingLeft: 20,
    paddingRight: 20,
    width: '40%',
    flexWrap: 'wrap',
}));

export default function Game({info, playerData}: Props) {
    return <>
        <FlexBox>
            <TeamFlexBox flexGrow={1} flexShrink={1} flexBasis={'auto'}>
                {info.teamA.map((player, id) =>
                    <GamePlayer
                        key={`teamA_${id}`}
                        info={player}
                        data={playerData.get(player.accountId)}
                        isTeamA={true}
                    />)}
            </TeamFlexBox>
            <Tooltip title={info.map} arrow>
                <Box flexBasis={120} style={{
                    backgroundImage: `url(${mapMiniPic(info.map)})`,
                    backgroundSize: 'cover',
                    borderColor: 'white',
                    borderWidth: 2,
                    borderStyle: 'solid',
                    borderRadius: 8,
                    textShadow: '2px 2px white',
                }}>
                    <Typography variant={'h3'}>{info.teamAScore} - {info.teamBScore}</Typography>
                </Box>
            </Tooltip>
            <TeamFlexBox flexGrow={1} flexShrink={1} flexBasis={'auto'}>
                {info.teamB.map((player, id) =>
                    <GamePlayer
                        key={`teamB_${id}`}
                        info={player}
                        data={playerData.get(player.accountId)}
                        isTeamA={false}
                    />)}
            </TeamFlexBox>
        </FlexBox>
    </>;
}

interface PlayerProps {
    info: GamePlayerData;
    data?: PlayerData;
    isTeamA: boolean;
}

export function GamePlayer({info, data, isTeamA}: PlayerProps) {
    let transformOuter, transformInner, borderColor;
    if (isTeamA) {
        transformOuter = 'skewX(-15deg)';
        transformInner = 'skewX(15deg)'
        borderColor = 'blue';
    } else {
        transformOuter = 'skewX(15deg)';
        transformInner = 'skewX(-15deg)'
        borderColor = 'red';
    }
    const handle = data?.handle ?? "UNKNOWN";

    return <>
        <Tooltip title={handle} arrow>
            <ButtonBase
                focusRipple
                style={{
                    width: 80,
                    height: 50,
                    transform: transformOuter,
                    overflow: 'hidden',
                    borderColor: borderColor,
                    borderWidth: 2,
                    borderStyle: 'solid',
                    borderRadius: 4,
                    backgroundColor: '#333',
                    margin: 2,
                }}
            >
                <div
                    style={{
                        transform: transformInner,
                        width: '115%',
                        height: '100%',
                        flex: 'none',
                    }}
                >
                    <BgImage style={{
                        backgroundImage: `url(${characterIcon(info.characterType)})`,
                    }} />
                </div>
            </ButtonBase>
        </Tooltip>
    </>;
}