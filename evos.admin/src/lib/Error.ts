export interface EvosError {
    text: string;
    description?: string;
}

export function processError(error: any, setError: (e: EvosError) => void, navigate: (url: string) => void) {
    if (error.response?.status === 401) {
        navigate("/login");
    }
    else if (error.response?.data?.message) {
        setError({text: error.response.data.message})
    }
    else if (error.response?.status === 404) {
        setError({text: "Not found"});
    }
    else if (error.response?.status === 403) {
        setError({text: "Access denied"});
    }
    else if (error.response?.status === 400) {
        setError({text: "Bad request"});
    }
    else if (error.response?.status === 405) {
        setError({text: "Method not allowed"});
    }
    else if (!error.response || error.response?.status === 500 || error.response?.status === 502) {
        setError({text: "Service unavailable"});
    }
    else {
        setError({text: "Unknown error"});
    }
}