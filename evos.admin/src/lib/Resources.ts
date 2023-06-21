import {CharacterType, MapType} from "./Evos";

export enum BannerType {
    background = "Background",
    foreground = "Foreground"
}

export function playerBanner(type: BannerType, id: number) {
    return `/banners/${type}/${id}.png`;
}

export function characterIcon(characterType: CharacterType) {
    return `/characters/icons/${characterType}.png`;
}

export function mapMiniPic(map: MapType) {
    return `/maps/mini/${map}.png`;
}


