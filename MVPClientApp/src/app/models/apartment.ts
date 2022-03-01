export class Apartment {
    id: string;
    name: string;
    description: string;
    floorAreaSize: number;
    pricePerMonth: number;
    numberOfRooms: number;
    longitude: number;
    latitude: number;
    isAvailable: boolean = false;
    addedDated: Date;
    realtorUserId: string;
}