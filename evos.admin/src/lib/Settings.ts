import {toMap} from "./Evos";

export interface SettingsData {
    key: string;
    defaultValue: any;
}

export enum SettingsKey {
    updateInBackground = 'updateInBackground',
}

const settingsList: SettingsData[] = [
    { key: SettingsKey.updateInBackground, defaultValue: false}
]

export const Settings = toMap(
    settingsList,
    s => s.key,
    s => s)
