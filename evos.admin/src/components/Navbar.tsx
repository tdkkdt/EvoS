import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import {BannerType, playerBanner} from "../lib/Resources";
import {useNavigate} from "react-router-dom";

const pages = [
    { text: "Status", url: '/' },
    { text: "Admin panel", url: '/admin' },
];

export default function NavBar() {
    const navigate = useNavigate();

    return (
        <AppBar position="static">
            <Container maxWidth="xl">
                <Toolbar disableGutters>
                    <Avatar alt="logo" src={playerBanner(BannerType.foreground, 65)} sx={{ width: 64, height: 64 }}/>
                    <Typography
                        variant="h5"
                        noWrap
                        component="a"
                        href=""
                        sx={{
                            mr: 2,
                            flexGrow: 1,
                            fontWeight: 700,
                            color: 'inherit',
                            textDecoration: 'none',
                        }}
                    >
                        Atlas Reactor
                    </Typography>
                    <Box sx={{ flexGrow: 1, display: 'flex' }}>
                        {pages.map((page) => (
                            <Button
                                key={page.text}
                                onClick={() => navigate(page.url)}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                {page.text}
                            </Button>
                        ))}
                    </Box>
                    <Box sx={{ flexGrow: 0 }}>
                        <Avatar alt="TODO" src={playerBanner(BannerType.foreground, 65)} />
                    </Box>
                </Toolbar>
            </Container>
        </AppBar>
    );
}