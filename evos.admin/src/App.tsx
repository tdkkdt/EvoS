import React from 'react';
import './App.css';
import StatusPage from "./components/StatusPage";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./components/LoginPage";
import NavBar from "./components/Navbar";
import AdminPage from "./components/AdminPage";

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
