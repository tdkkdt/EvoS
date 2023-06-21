import axios from "axios";

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
    bannerBg: number;
    bannerFg: number;
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
    map: MapType;
    status: string;
    turn: number;
    teamAScore: number;
    teamBScore: number;
}

export interface GamePlayerData {
    accountId: number;
    characterType: CharacterType;
}

export enum CharacterType {
    None = 'None',
    BattleMonk = 'BattleMonk',
    BazookaGirl = 'BazookaGirl',
    DigitalSorceress = 'DigitalSorceress',
    Gremlins = 'Gremlins',
    NanoSmith = 'NanoSmith',
    RageBeast = 'RageBeast',
    RobotAnimal = 'RobotAnimal',
    Scoundrel = 'Scoundrel',
    Sniper = 'Sniper',
    SpaceMarine = 'SpaceMarine',
    Spark = 'Spark',
    TeleportingNinja = 'TeleportingNinja',
    Thief = 'Thief',
    Tracker = 'Tracker',
    Trickster = 'Trickster',
    PunchingDummy = 'PunchingDummy',
    Rampart = 'Rampart',
    Claymore = 'Claymore',
    Blaster = 'Blaster',
    FishMan = 'FishMan',
    Exo = 'Exo',
    Soldier = 'Soldier',
    Martyr = 'Martyr',
    Sensei = 'Sensei',
    PendingWillFill = 'PendingWillFill',
    Manta = 'Manta',
    Valkyrie = 'Valkyrie',
    Archer = 'Archer',
    TestFreelancer1 = 'TestFreelancer1',
    TestFreelancer2 = 'TestFreelancer2',
    Samurai = 'Samurai',
    Gryd = 'Gryd',
    Cleric = 'Cleric',
    Neko = 'Neko',
    Scamp = 'Scamp',
    FemaleWillFill = 'FemaleWillFill',
    Dino = 'Dino',
    Iceborg = 'Iceborg',
    Fireborg = 'Fireborg',
    Last = 'Last',
}

export enum MapType {
    CargoShip_Deathmatch = 'CargoShip_Deathmatch',
    Casino01_Deathmatch = 'Casino01_Deathmatch',
    EvosLab_Deathmatch = 'EvosLab_Deathmatch',
    Oblivion_Deathmatch = 'Oblivion_Deathmatch',
    Reactor_Deathmatch = 'Reactor_Deathmatch',
    RobotFactory_Deathmatch = 'RobotFactory_Deathmatch',
    Skyway_Deathmatch = 'Skyway_Deathmatch',
}

export function getStatus(authHeader: string) {
    return axios.get<Status>("/api/lobby/status", { headers: {'Authorization': authHeader} });
}

export function login(username: string, password: string) {
    return axios.post<string>("/api/login", { UserName: username, Password: password });
}
