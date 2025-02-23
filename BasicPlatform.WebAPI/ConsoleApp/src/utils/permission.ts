export default {
  /** 外部页面管理 */
  externalPage: {
    /** 读取列表 */
    getPagingAsync: 'ExternalPageController_GetPagingAsync',
    /** 详情 */
    getAsync: 'externalPage:detail',
    /** 创建 */
    postAsync: 'ExternalPageController_PostAsync',
    /** 编辑 */
    putAsync: 'ExternalPageController_PutAsync',
    /** 删除 */
    deleteAsync: 'ExternalPageController_DeleteAsync',
  },
  /** 组织架构管理 */
  organization: {
    /** 读取列表 */
    getPagingAsync: 'OrganizationController_GetPagingAsync',
    /** 详情 */
    getAsync: 'org:detail',
    /** 创建 */
    postAsync: 'OrganizationController_PostAsync',
    /** 编辑 */
    putAsync: 'OrganizationController_PutAsync',
    /** 状态变更 */
    statusChangeAsync: 'OrganizationController_StatusChangeAsync',
  },
  /** 职位管理 */
  position: {
    /** 读取列表 */
    getPagingAsync: 'PositionController_GetPagingAsync',
    /** 详情 */
    getAsync: 'position:detail',
    /** 创建 */
    postAsync: 'PositionController_PostAsync',
    /** 更新 */
    putAsync: 'PositionController_PutAsync',
    /** 状态变更 */
    statusChangeAsync: 'PositionController_StatusChangeAsync',
  },
  /** 角色管理 */
  role: {
    /** 读取列表 */
    getPagingAsync: 'RoleController_GetPagingAsync',
    /** 详情 */
    getAsync: 'role:detail',
    /** 创建 */
    postAsync: 'RoleController_PostAsync',
    /** 更新 */
    putAsync: 'RoleController_PutAsync',
    /** 状态变更 */
    statusChangeAsync: 'RoleController_StatusChangeAsync',
    /** 分配资源 */
    assignResourcesAsync: 'RoleController_AssignResourcesAsync',
    /** 分配权限 */
    assignDataPermissionsAsync: 'RoleController_AssignDataPermissionsAsync',
    /** 分配用户 */
    assignUsersAsync: 'RoleController_AssignUsersAsync',
  },
  /** 服务器信息 */
  server: {
    /** 读取服务器信息 */
    get: 'ServerController_Get',
  },
  /** 租户管理 */
  tenant: {
    /** 读取列表 */
    getPagingAsync: 'TenantController_GetPagingAsync',
    /** 详情 */
    getAsync: 'tenant:detail',
    /** 创建 */
    postAsync: 'TenantController_PostAsync',
    /** 更新 */
    putAsync: 'TenantController_PutAsync',
    /** 状态变更 */
    statusChangeAsync: 'TenantController_StatusChangeAsync',
    /** 分配资源 */
    assignResourcesAsync: 'TenantController_AssignResourcesAsync',
    /** 初始化 */
    initAsync: 'TenantController_InitAsync',
    /** 同步数据库 */
    syncStructureAsync: 'TenantController_SyncStructureAsync',
  },
  /** 员工访问记录 */
  userAccessRecord: {
    /** 读取列表 */
    getPagingAsync: 'UserAccessRecordController_GetPagingAsync',
  },
  /** 用户管理 */
  user: {
    /** 读取列表 */
    getPagingAsync: 'UserController_GetPagingAsync',
    /** 详情 */
    getAsync: 'user:detail',
    /** 创建 */
    postAsync: 'UserController_PostAsync',
    /** 更新 */
    putAsync: 'UserController_PutAsync',
    /** 状态变更 */
    statusChangeAsync: 'UserController_StatusChangeAsync',
    /** 分配资源 */
    assignResourcesAsync: 'UserController_AssignResourcesAsync',
    /** 分配权限 */
    assignDataPermissionsAsync: 'UserController_AssignDataPermissionsAsync',
    /** 重置密码 */
    resetPasswordAsync: 'UserController_ResetPasswordAsync',
    /** 下拉列表 */
    getSelectListAsync: 'UserController_GetSelectListAsync',
    /** 读取资源 */
    getResourcesAsync: 'UserController_GetResourcesAsync',
    /** 读取外部页面列表 */
    getExternalPagesAsync: 'UserController_GetExternalPagesAsync',
  },
  /** 应用管理 */
  application: {
    /** 读取列表 */
    getPagingAsync: 'ApplicationController_GetPagingAsync',
    /** 读取列表 */
    getListAsync: 'ApplicationController_GetListAsync',
    /** 详情 */
    getAsync: 'application:detail',
    /** 创建 */
    postAsync: 'ApplicationController_PostAsync',
    /** 编辑 */
    putAsync: 'ApplicationController_PutAsync',
    /** 状态变更 */
    statusChangeAsync: 'ApplicationController_StatusChangeAsync',
  },
  /** 资源管理 */
  resource: {
    /** 同步资源 */
    syncAsync: 'ResourceController_SyncAsync',
    /** 读取子应用资源 */
    getSubAppResourcesAsync: 'ResourceController_GetSubAppResourcesAsync',
  },
};