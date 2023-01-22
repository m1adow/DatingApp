import { Photo } from "./photo";

export interface PhotoApprove {
    username: string;
    knownAs: string;
    photos: Photo[];
}

export interface ApprovedPhoto {
    username: string;   
    photoId: number;
    isApproved: boolean;
}