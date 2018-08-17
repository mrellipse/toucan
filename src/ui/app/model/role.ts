export const DefaultRole = <IRole>{
  assignedUserCount: 0,
  createdByUser: null,
  createdOn: null,
  enabled: true,
  isSystemRole: false,
  name: null,
  parentRoleId: null,
  roleId: null,
  securityClaims: []
};

interface IAuditUser {
  userId: number;
  displayName: string;
}

export interface IRole {
  assignedUserCount: number;
  createdByUser: IAuditUser;
  createdOn: Date;
  enabled: boolean;
  isSystemRole: boolean;
  lastUpdatedByUser?: IAuditUser;
  lastUpdatedOn?: Date;
  name: string;
  parentRoleId: string;
  roleId: string;
  securityClaims: IRoleSecurityClaim[];
}

export interface IRoleSecurityClaim {
  inherited?: boolean;
  roleId?: string;
  securityClaim?: ISecurityClaim;
  securityClaimId: string;
  unwatch?: () => void;
  value: string;
  values?: any[];
}

export interface ISecurityClaim {
  securityClaimId: string;
  description: string;
  enabled: boolean;
  origin: string;
  validationPattern: string;
}
