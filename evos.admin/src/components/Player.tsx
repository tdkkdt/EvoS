import {PlayerData} from "../lib/Evos";
import {Button, ButtonBase, styled, Typography} from "@mui/material";

interface Props {
    info?: PlayerData;
}

const ImageSrc = styled('span')({
    position: 'absolute',
    left: 0,
    right: 0,
    top: 0,
    bottom: 0,
    backgroundSize: 'cover',
    backgroundPosition: 'center 40%',
    zIndex: -1000,
});

const Image = styled('span')(({ theme }) => ({
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
    let username = 'UNKNOWN', discriminator;
    if (info) {
        [username, discriminator] = info.handle.split('#', 2)
    }

    return <>
        <ButtonBase
            focusRipple
            key={info?.handle}
            style={{
                width: 480,
                height: 104,
                transform: 'skewX(-15deg)',
                overflow: 'hidden',
                border: '4px solid black'
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
                <ImageSrc style={{
                    backgroundImage: `url(/banners/Background/95.png)`,
                }} />
                <ImageSrc style={{
                    marginTop: '-3%',
                    marginLeft: '-3%',
                    backgroundImage: `url(/banners/Foreground/65.png)`,
                    width: '34%',
                }} />
                <Image>
                    <Typography component={'span'} style={{ fontSize: '1em' }}>{username}</Typography>
                    {discriminator && <Typography component={'span'} style={{ fontSize: '0.8em' }}>#{discriminator}</Typography>}
                </Image>
            </div>
        </ButtonBase>
    </>;
}

export default Player;