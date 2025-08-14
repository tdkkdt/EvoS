import { useState, useEffect } from "react";
import {SettingsData} from "./Settings";

const STORAGE_EVENT_NAME = 'onLocalStorageChange';

function getStorageValue(key: string, defaultValue: any) {
    const saved = localStorage.getItem(key);
    const initial = saved === null ? defaultValue : JSON.parse(saved);
    return initial || defaultValue;
}

function dispatchStorageEvent(key: string, newValue: any) {
    window.dispatchEvent(
        new CustomEvent(STORAGE_EVENT_NAME, {
            detail: { key, newValue }
        })
    );
}

export const useLocalStorage = ({key, defaultValue}: SettingsData) => {
    const [value, setValue] = useState(() => {
        return getStorageValue(key, defaultValue);
    });

    useEffect(() => {
        const handleStorageChange = (e: CustomEvent) => {
            if (e.detail.key === key) {
                setValue(e.detail.newValue);
            }
        };
        window.addEventListener(STORAGE_EVENT_NAME, handleStorageChange as EventListener);

        return () => {
            window.removeEventListener(STORAGE_EVENT_NAME, handleStorageChange as EventListener);
        };
    }, [key]);

    const setStorageValue = (newValue: any) => {
        const valueToStore = newValue instanceof Function ? newValue(value) : newValue;
        setValue(valueToStore);
        localStorage.setItem(key, JSON.stringify(valueToStore));
        dispatchStorageEvent(key, valueToStore);
    };

    return [value, setStorageValue];
};