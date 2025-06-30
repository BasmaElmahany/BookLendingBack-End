export interface LoginDto {
    email: string;
    password: string;
  }
  
  export interface RegisterUserDto {
    fullName: string;
    email: string;
    password: string;
    role : string ;
  }


  export interface TokenPayload {
  sub: string;
  email: string;
  role: string;
  [key: string]: any;
}
