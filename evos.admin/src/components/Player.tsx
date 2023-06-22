import {PlayerData} from "../lib/Evos";
import {ButtonBase, Typography} from "@mui/material";
import {BgImage, ImageTextWrapper} from "./BasicComponents";
import {BannerType, playerBanner} from "../lib/Resources";

interface Props {
    info?: PlayerData;
}

function Player({info}: Props) {
    let username = 'UNKNOWN', discriminator;
    if (info) {
        [username, discriminator] = info.handle.split('#', 2)
    }

    return <>
        <ButtonBase
            focusRipple
            key={info?.handle}
            style={{
                width: 240,
                height: 52,
                fontSize: '8px',
                transform: 'skewX(-15deg)',
                overflow: 'hidden',
                border: '2px solid black'
            }}
        >
            <div
                style={{
                    transform: 'skewX(15deg)',
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
                        textShadow: '1px 1px black',
                    }}>
                    <Typography component={'span'} style={{ fontSize: '1em' }}>{username}</Typography>
                    {discriminator && <Typography component={'span'} style={{ fontSize: '0.8em' }}>#{discriminator}</Typography>}
                </ImageTextWrapper>
            </div>
        </ButtonBase>
    </>;
}

export default Player;