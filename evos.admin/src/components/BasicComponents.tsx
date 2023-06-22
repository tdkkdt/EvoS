import {Box, styled} from "@mui/material";

export const BgImage = styled('span')({
    position: 'absolute',
    left: 0,
    right: 0,
    top: 0,
    bottom: 0,
    backgroundSize: 'cover',
    backgroundPosition: 'center 40%',
    zIndex: -1000,
});

export const FlexBox = styled(Box)(({ theme }) => ({
    display: 'inline-flex',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    width: '100%',
}));