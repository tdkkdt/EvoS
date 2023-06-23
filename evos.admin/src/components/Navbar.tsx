import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import {BannerType, logo, playerBanner} from "../lib/Resources";
import {NavLink} from "react-router-dom";
import {useAuthUser, useIsAuthenticated} from "react-auth-kit";
import {Stack, styled, Typography} from "@mui/material";

const pages = [
    { text: "Status", url: '/' },
    { text: "Admin panel", url: '/admin' },
];

export const NavBarLink = styled(NavLink)({
    textDecoration: 'none',
});

export const NavBarText = styled(Typography)({
    color: 'white',
    display: 'block',
    textDecoration: 'none',
    fontFamily: '"Roboto","Helvetica","Arial",sans-serif',
    fontWeight: 500,
    fontSize: '0.875rem',
    lineHeight: 1.75,
    letterSpacing: '0.02857em',
    textTransform: 'uppercase',
    minWidth: 64,
    padding: '6px 8px',
});


export default function NavBar() {
    const isAuthenticated = useIsAuthenticated();
    const auth = useAuthUser();

    return (
        <AppBar position="static">
            <Container maxWidth="xl">
                <Toolbar disableGutters>
                    <Avatar alt="logo" variant="square" src={logo()} sx={{ flexShrink: 1, width: 255, height: 40 }}/>
                    <Stack direction={"row"} alignItems="center" sx={{ flexGrow: 1, display: 'flex', justifyContent: 'space-evenly' }}>
                        {isAuthenticated() && pages.map((page) => (
                            <NavBarLink key={page.text} to={page.url}><NavBarText>{page.text}</NavBarText></NavBarLink>
                        ))}

                    </Stack>
                    <Box sx={{ flexGrow: 0 }}>
                        {isAuthenticated() && <Stack direction={"row"} alignItems="center">
                            <NavBarText>{auth()?.handle}</NavBarText>
                            <Avatar
                                alt="Avatar"
                                src={playerBanner(BannerType.foreground, auth()?.banner ?? 65)}
                                sx={{ width: 64, height: 64 }}
                            />
                        </Stack>}
                        {!isAuthenticated() && <NavBarLink to='/login' style={(active) => active && { display: 'none' }}>
                            <NavBarText>Log in</NavBarText>
                        </NavBarLink>}
                    </Box>
                </Toolbar>
            </Container>
        </AppBar>
    );
}