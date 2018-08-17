export const DefaultUser = <IUser>{
  authenticated: false,
  claims: {},
  cultureName: "en",
  displayName: null,
  email: null,
  name: null,
  username: null,
  roles: [],
  timeZoneId: "AUS Eastern Standard Time",
  verified: false
};

export interface IUser {
  authenticated: boolean;
  claims: {};
  cultureName?: string;
  createdOn?: Date;
  displayName?: string;
  email?: string;
  enabled?: boolean;
  name?: string;
  roles: string[];
  username: string;
  verified?: boolean;
  exp?: Date;
  userId?: number;
  timeZoneId?: string;
}
export const SecurityRoleClaims = {
  Admin: "admin",
  Client: "client",
  SiteAdmin: "siteadmin",
  User: "user"
};

export const SecurityClaims = {
  Api: "api",
  SiteSettings: "sitesettings",
  UserSettings: "usersettings"
};
