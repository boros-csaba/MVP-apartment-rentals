export class User {
    userId: string;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
    password: string;
    confirmedPassword: string;
    isBlocked: boolean;
}