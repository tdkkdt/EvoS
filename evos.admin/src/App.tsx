import React, {useEffect} from 'react';
import './App.css';
import StatusPage from "./components/pages/StatusPage";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./components/pages/LoginPage";
import NavBar from "./components/Navbar";
import AdminPage from "./components/pages/AdminPage";
import ProfilePage from "./components/pages/ProfilePage";
import {colors, createTheme, CssBaseline, Paper, ThemeProvider} from "@mui/material";

const theme = createTheme({
    components: {
        MuiCssBaseline: {
            styleOverrides: {
                html: {
                    margin: 0,
                    minHeight: '100vh',
                },
                body: {
                    backgroundColor: '#000',
                    color: 'white',
                    textAlign: 'center',
                    margin: 0,
                    minHeight: '100vh',
                }
            }
        }
    },
    size: {
        basicWidth: 600,
    },
    palette: {
        // mode: 'dark',
        primary: {
            main: colors.blue[700],
        },
        secondary: {
            main: colors.orange[500],
        },
        background: {
            default: '#000',
            paper: '#282c34',
        },
        action : {
            active: '#fff',
            hover: 'rgba(255, 255, 255, 0.08)',
            selected: 'rgba(255, 255, 255, 0.16)',
            disabled: 'rgba(255, 255, 255, 0.3)',
            disabledBackground: 'rgba(255, 255, 255, 0.12)',
        },
        text: {
            primary: '#fff',
            secondary: 'rgba(255, 255, 255, 0.7)',
            disabled: 'rgba(255, 255, 255, 0.5)',
        },
        divider: 'rgba(255, 255, 255, 0.12)',
        teamA: {
            main: colors.blue[500],
        },
        teamB: {
            main: colors.red[500],
        }
    }
});

interface PageProps {
    title: string;
    children?: React.ReactNode;
}

function Page(props: PageProps) {
    useEffect(() => {
        document.title = props.title || "Atlas Reactor";
    }, [props.title]);
    return <>{props.children}</>;
}

function App() {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <BrowserRouter>
                <NavBar/>
                <Paper sx={{
                    width: 'calc(100% - 16px)',
                    margin: 'auto',
                    borderTopLeftRadius: 0,
                    borderTopRightRadius: 0,
                }}>
                    <Routes>
                        <Route path="/" element={<Page title={"Lobby status"}><StatusPage/></Page>}/>
                        <Route path="/login" element={<Page title={"Atlas Reactor: Login"}><LoginPage/></Page>}/>
                        <Route path="/admin" element={<Page title={"Admin panel"}><AdminPage/></Page>}/>
                        <Route path="/account/:accountId" element={<Page title={"Account"}><ProfilePage/></Page>}/>
                    </Routes>
                </Paper>
            </BrowserRouter>
        </ThemeProvider>
    );
}

export default App;
