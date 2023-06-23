export interface EvosError {
    text: string;
    description?: string;
}

export function processError(error: any, setError: (e: EvosError) => void, navigate: (url: string) => void) {
    if (error.response?.status === 401) {
        navigate("/login");
    }
    else if (error.response?.status === 404) {
        setError({text: "Not found"});
    }
    else if (!error.response || error.response?.status === 500) {
        setError({text: "Service unavailable"});
    }
    else {
        setError({text: "Unknown error"});
    }
}