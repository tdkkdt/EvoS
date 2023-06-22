import {CharacterType, PlayerData} from "../lib/Evos";
import {ButtonBase, Tooltip} from "@mui/material";
import {BgImage} from "./BasicComponents";
import {characterIcon} from "../lib/Resources";
import React from "react";


interface CharacterIconProps {
    characterType: CharacterType;
    data?: PlayerData;
    isTeamA: boolean;
    rightSkew?: boolean;
}

export function CharacterIcon({characterType, data, isTeamA, rightSkew}: CharacterIconProps) {
    let transformOuter, transformInner;
    if (isTeamA || rightSkew) {
        transformOuter = 'skewX(-15deg)';
        transformInner = 'skewX(15deg)';
    } else {
        transformOuter = 'skewX(15deg)';
        transformInner = 'skewX(-15deg)';
    }
    const borderColor = isTeamA ? 'blue' : 'red';
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
                        backgroundImage: `url(${characterIcon(characterType)})`,
                    }} />
                </div>
            </ButtonBase>
        </Tooltip>
    </>;
}