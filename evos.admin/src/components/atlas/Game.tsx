import {GameData, GamePlayerData, PlayerData} from "../../lib/Evos";
import {Box, Stack, styled, Tooltip, Typography} from "@mui/material";
import {FlexBox} from "../generic/BasicComponents";
import {mapMiniPic} from "../../lib/Resources";
import Player from "./Player";
import React from "react";
import {CharacterIcon} from "./CharacterIcon";

export const TeamFlexBox = styled(FlexBox)(({ theme }) => ({
    paddingLeft: 20,
    paddingRight: 20,
    width: '40%',
    flexWrap: 'wrap',
}));


interface TeamProps {
    caption?: string;
    info: GamePlayerData[];
    isTeamA: boolean;
    playerData: Map<number, PlayerData>;
}

function TeamRow({info, isTeamA, playerData}: TeamProps) {
    return (
        <TeamFlexBox flexGrow={1} flexShrink={1} flexBasis={'auto'}>
            {info.map((player, id) =>
                <CharacterIcon
                    key={`teamA_${id}`}
                    characterType={player.characterType}
                    data={playerData.get(player.accountId)}
                    isTeamA={isTeamA}
                />)}
        </TeamFlexBox>
    )
}

function Team({caption, info, isTeamA, playerData}: TeamProps) {
    return (
        <Stack>
            {caption && <Typography variant={'h5'}>{caption}</Typography>}
            {info.map((p, id) =>
                <Stack key={`teamA_${id}`} direction={isTeamA ? 'row' : 'row-reverse'}>
                    <Player info={playerData.get(p.accountId)} />
                    <CharacterIcon
                        characterType={p.characterType}
                        data={playerData.get(p.accountId)}
                        isTeamA={isTeamA}
                        rightSkew
                    />
                </Stack>
            )}
        </Stack>
    )
}

interface Props {
    info: GameData;
    playerData: Map<number, PlayerData>;
}

export default function Game({info, playerData}: Props) {
    const A = {
        caption: "Team Blue",
        info: info.teamA,
        playerData: playerData,
        isTeamA: true,
    }
    const B = {
        caption: "Team Orange",
        info: info.teamB,
        playerData: playerData,
        isTeamA: false,
    }

    return <>
        <Stack width={'100%'}>
            <FlexBox>
                <TeamRow {...A} />
                <Tooltip title={`${info.map} ${info.ts}`} arrow>
                    <Box flexBasis={120} style={{
                        backgroundImage: `url(${mapMiniPic(info.map)})`,
                        backgroundSize: 'cover',
                        borderColor: 'white',
                        borderWidth: 2,
                        borderStyle: 'solid',
                        borderRadius: 8,
                    }}>
                        <Typography variant={'h3'}>
                            <span style={{ textShadow: '2px 2px blue' }}>{info.teamAScore}</span>
                            <span> - </span>
                            <span style={{ textShadow: '2px 2px red' }}>{info.teamBScore}</span>
                        </Typography>
                        <Typography
                            variant={'caption'}
                            component={'div'}
                            style={{
                                textShadow: '1px 1px 2px black, -1px -1px 2px black, 1px -1px 2px black, -1px 1px 2px black',
                                marginTop: -10,
                            }}>
                            {info.status === 'Started' ? `Turn ${info.turn}` : info.status}
                        </Typography>
                    </Box>
                </Tooltip>
                <TeamRow {...B} />
            </FlexBox>
            <FlexBox style={{justifyContent : 'space-around', flexWrap: 'wrap'}} >
                <Team {...A} />
                <Team {...B} />
            </FlexBox>
        </Stack>
    </>;
}