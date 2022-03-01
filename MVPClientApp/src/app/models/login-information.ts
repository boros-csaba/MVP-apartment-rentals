export class LoginInformation {
    userId: string;
    email: string;
    firstName: string;
    lastName: string;
    isRealtor: boolean;
    isAdmin: boolean;
    token: string;
    tokenExpiration: Date;
    refreshToken: string;
    refreshTokenExpiration: Date;
}