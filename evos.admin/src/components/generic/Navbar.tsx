import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import {BannerType, logo, playerBanner} from "../../lib/Resources";
import {NavLink, useNavigate} from "react-router-dom";
import {useAuthHeader, useAuthUser, useIsAuthenticated, useSignOut} from "react-auth-kit";
import {Menu, MenuItem, Stack, styled, TextField, Typography} from "@mui/material";
import {useState} from "react";
import {findPlayer} from "../../lib/Evos";
import {EvosError, processError} from "../../lib/Error";
import ErrorDialog from "./ErrorDialog";

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
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const [query, setQuery] = useState<string>("");
    const [error, setError] = useState<EvosError>();

    const isAuthenticated = useIsAuthenticated();
    const signOut = useSignOut();
    const auth = useAuthUser();
    const authHeader = useAuthHeader();
    const navigate = useNavigate();

    const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleLogOut = () => {
        handleClose();
        signOut();
        navigate('/login');
    };

    const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(event.target.value);
    }

    const handleSearchKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key !== "Enter" || !query) return;

        event.preventDefault();
        findPlayer(new AbortController(), authHeader(), query)
            .then((resp) => {
                navigate(`/account/${resp.data.accountId}`);
            })
            .catch((error) => processError(error, setError, navigate))
    }

    return (
        <AppBar position="static">
            {error && <ErrorDialog error={error} onDismiss={() => setError(undefined)} />}
            <Container maxWidth="xl">
                <Toolbar disableGutters>
                    <Avatar alt="logo" variant="square" src={logo()} sx={{ flexShrink: 1, width: 255, height: 40 }}/>
                    <Stack direction={"row"} alignItems="center" sx={{ flexGrow: 1, display: 'flex', justifyContent: 'space-evenly' }}>
                        {isAuthenticated() && pages.map((page) => (
                            <NavBarLink key={page.text} to={page.url}><NavBarText>{page.text}</NavBarText></NavBarLink>
                        ))}
                        {isAuthenticated() &&
                            <TextField
                                id="account-search"
                                type="search"
                                label="Find player"
                                variant="outlined"
                                value={query}
                                onChange={handleSearchChange}
                                onKeyDown={handleSearchKeyDown}
                            />}
                    </Stack>
                    <Box sx={{ flexGrow: 0 }}>
                        {isAuthenticated() && <>
                            <Stack direction={"row"} alignItems="center" sx={{cursor: "pointer"}} onClick={handleMenu}>
                                <NavBarText>{auth()?.handle}</NavBarText>
                                <Avatar
                                    alt="Avatar"
                                    src={playerBanner(BannerType.foreground, auth()?.banner ?? 65)}
                                    sx={{ width: 64, height: 64 }}
                                />
                            </Stack>
                            <Menu
                                id="menu-appbar"
                                anchorEl={anchorEl}
                                anchorOrigin={{
                                    vertical: 'bottom',
                                    horizontal: 'right',
                                }}
                                keepMounted
                                transformOrigin={{
                                    vertical: 'top',
                                    horizontal: 'right',
                                }}
                                open={Boolean(anchorEl)}
                                onClose={handleClose}
                            >
                                <MenuItem onClick={handleLogOut}>Log out</MenuItem>
                            </Menu>
                        </>}
                        {!isAuthenticated() && <NavBarLink to='/login' style={(active) => active && { display: 'none' }}>
                            <NavBarText>Log in</NavBarText>
                        </NavBarLink>}
                    </Box>
                </Toolbar>
            </Container>
        </AppBar>
    );
}