import {PlayerData} from "../../lib/Evos";
import {ButtonBase, styled, Typography, useTheme} from "@mui/material";
import {BgImage} from "../generic/BasicComponents";
import {BannerType, playerBanner} from "../../lib/Resources";
import React from "react";
import {useNavigate} from "react-router-dom";

interface Props {
    info?: PlayerData;
}

const ImageTextWrapper = styled('span')(({ theme }) => ({
    position: 'absolute',
    top: '3%',
    left: '28%',
    color: theme.palette.common.white,
    textAlign: 'left',
    fontSize: '2.5em',
    fontStretch: 'condensed',
    width: '100%',
}));

function Player({info}: Props) {
    let username = 'OFFLINE', discriminator;
    if (info) {
        [username, discriminator] = info.handle.split('#', 2)
    }

    const navigate = useNavigate();
    const theme = useTheme();

    const handleClick = () => {
        if (!info) {
            return;
        }
        navigate(`/account/${info.accountId}`);
    }

    return <>
       <ButtonBase
            focusRipple
            key={info?.handle}
            onClick={handleClick}
            style={{
                width: 240,
                height: 52,
                fontSize: '8px',
                transform: theme.transform.skewA,
                overflow: 'hidden',
                border: '2px solid black'
            }}
        >
            <div
                style={{
                    transform: theme.transform.skewB,
                    width: '106%',
                    height: '100%',
                    flex: 'none',
                }}
            >
                <BgImage style={{
                    backgroundImage: info && `url(${playerBanner(BannerType.background, info.bannerBg)})`,
                }} />
                <BgImage style={{
                    marginTop: '-3%',
                    marginLeft: '-3%',
                    backgroundImage: info && `url(${playerBanner(BannerType.foreground, info.bannerFg)})`,
                    width: '35%',
                }} />
                <ImageTextWrapper
                    style={{
                        textShadow: '1px 1px 2px black, -1px -1px 2px black, 1px -1px 2px black, -1px 1px 2px black',
                    }}>
                    <Typography component={'span'} style={{ fontSize: '1em' }}>{username}</Typography>
                    {discriminator && <Typography component={'span'} style={{ fontSize: '0.8em' }}>#{discriminator}</Typography>}
                </ImageTextWrapper>
            </div>
        </ButtonBase>
    </>;
}

export default Player;