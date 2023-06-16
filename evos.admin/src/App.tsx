import React from 'react';
import './App.css';
import StatusPage from "./components/StatusPage";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import LoginPage from "./components/LoginPage";

function App() {
  return (
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<StatusPage />} />
          <Route path="/login" element={<LoginPage />} />
        </Routes>
      </BrowserRouter>

  );
}

export default App;
