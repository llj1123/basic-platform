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
  /** 服务器信息 */
  server: {
    /** 读取服务器信息 */
    get: 'ServerController_Get',
  },
  /** 资源管理 */
  resource: {
    /** 同步资源 */
    syncAsync: 'ResourceController_SyncAsync',
    /** 重置资源 */
    reinitializeAsync: 'ResourceController_ReinitializeAsync',
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
    /** 读取树形列表 */
    getTreeListAsync: 'OrganizationController_GetTreeListAsync',
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
  /** 员工访问记录 */
  userAccessRecord: {
    /** 读取列表 */
    getPagingAsync: 'UserAccessRecordController_GetPagingAsync',
  },
  /** 员工管理 */
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
    /** 重置密码 */
    resetPasswordAsync: 'UserController_ResetPasswordAsync',
  },
};