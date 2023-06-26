import React from 'react';
import './App.css';
import StatusPage from "./components/pages/StatusPage";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./components/pages/LoginPage";
import NavBar from "./components/Navbar";
import AdminPage from "./components/pages/AdminPage";

function App() {
    return (
        <BrowserRouter>
            <NavBar/>
            <Routes>
                <Route path="/" element={<StatusPage/>}/>
                <Route path="/login" element={<LoginPage/>}/>
                <Route path="/admin" element={<AdminPage/>}/>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
