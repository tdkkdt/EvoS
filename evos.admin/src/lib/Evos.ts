import axios from "axios";

const baseUrl = "http://localhost:3001";

export interface Status
{
    players: PlayerData[];
    groups: GroupData[];
    queues: QueueData[];
    servers: ServerData[];
    games: GameData[];
}

export interface PlayerData
{
    accountId: number;
    handle: string;
}

export interface GroupData
{
    groupId: number;
    accountIds: number[];
}

export interface QueueData
{
    type: string;
    groupIds: number[];
}

export interface ServerData
{
    id: string;
    name: string;
}

export interface GameData
{
    id: string;
    ts: string;
    server: string;
    teamA: GamePlayerData[];
    teamB: GamePlayerData[];
    map: string;
    status: string;
    turn: number;
    teamAScore: number;
    teamBScore: number;
}

export interface GamePlayerData {
    accountId: number;
    characterType: string;
}

export function getStatus(authHeader: string) {
    return axios.get<Status>(baseUrl + "/api/lobby/status", { headers: {'Authorization': authHeader} });
}

export function login(username: string, password: string) {
    return axios.post<string>(baseUrl + "/api/login", { UserName: username, Password: password });
}
