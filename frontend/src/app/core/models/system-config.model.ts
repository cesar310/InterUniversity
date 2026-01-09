// Enum ConfigValueType del backend (C#)
// 0 = Int, 1 = String, 2 = Boolean, 3 = Decimal
export enum ConfigValueType {
  Int = 0,
  String = 1,
  Boolean = 2,
  Decimal = 3
}

export interface SystemConfig {
  id: number;
  configKey: string;
  configValue: string;
  valueType: ConfigValueType;
  description: string;
  isEditable: boolean;
  updatedAt?: string;
}

export interface UpdateConfigRequest {
  value: string;
}

export interface ConfigAudit {
  id: number;
  configKey: string;
  oldValue: string;
  newValue: string;
  changedBy: string;
  changedAt: string;
}