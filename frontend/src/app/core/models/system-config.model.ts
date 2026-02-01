// Enum ConfigValueType del backend (C#)
// El backend serializa los enums como strings gracias a JsonStringEnumConverter
export enum ConfigValueType {
  Int = 'Int',
  String = 'String',
  Boolean = 'Boolean',
  Decimal = 'Decimal'
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
  oldValue: string | null;
  newValue: string;
  changedBy: string | null;
  changedAt: string;
}