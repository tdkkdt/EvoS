import {PaletteColor, PaletteColorOptions} from "@mui/material";
import React from "react";

declare module '@mui/material/styles' {
    interface Theme {
        size: {
            basicWidth: string;
        }
    }

    interface ThemeOptions {
        size: {
            basicWidth: React.CSSProperties['width'];
        }
    }

    interface Palette {
        teamA?: PaletteColor;
        teamB?: PaletteColor;
        header?: PaletteColor;
    }

    interface PaletteOptions {
        teamA?: PaletteColorOptions;
        teamB?: PaletteColorOptions;
        header?: PaletteColorOptions;
    }
}